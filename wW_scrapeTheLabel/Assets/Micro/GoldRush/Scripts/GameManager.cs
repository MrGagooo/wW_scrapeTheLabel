using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GoldRush
{
    public class GameManager : MicroMonoBehaviour
    {
        #region Affichage du Niveau
        public Texture2D levelTexture;
        Texture2D textureInstance;
        public SpriteRenderer levelRenderer;

        [Range(2, 10)]
        public float timeScaled;

        public float posOffset = 0.01f;
        int maxX;
        int maxY;
        Node[,] grid;

        Vector3 mousePos;
        Node curNode;
        Node prevNode;

        public Transform fillDebugObj;
        public bool addFill;
        public int pixelsOut;
        public int maxPixels;
        float f_t;
        float p_t;

        public Transform spawnTransform;
        [HideInInspector]
        public Node spawnNode;
        [HideInInspector]
        public Vector3 spawnPosition;

        #endregion

        public Color buildColor = Color.blue;
        public Color fillColor = Color.cyan;
        public float editRadius = 6;
        public bool overUIElement;

        bool applyTexture;

        public LineRenderer lineRnd;

        public GameObject dynamite;
        public Transform dynSpawn;

        Vector2 endMousePosTemp;
        bool currentPlayerState = false;

        public List<bool> etatFilonOr;
        int nbLance;
        public int radius = 50;

        public List<GameObject> levelsDesign;

        void Start()
        {
            CreateLevel();
            spawnNode = GetNodeFromWorldPos(spawnTransform.position);
            spawnPosition = GetWorldPosFromNode(spawnNode);
            
            if(Macro.Difficulty == 1)
            {
                levelsDesign[Random.Range(0, 3)].SetActive(true);
                etatFilonOr.Add(true);
            }
            else if(Macro.Difficulty == 2)
            {
                levelsDesign[Random.Range(0, 3) + 3].SetActive(true);
                etatFilonOr.Add(true);
                etatFilonOr.Add(true);
            }
            else
            {
                levelsDesign[Random.Range(0, 3) + 6].SetActive(true);
                etatFilonOr.Add(true);
                etatFilonOr.Add(true);
            }
            
            lineRnd.SetWidth(0.01f, 0.01f);
            lineRnd.SetPosition(0, dynSpawn.position);

            Macro.StartGame();
        }

        void CreateLevel()
        {
            maxX = levelTexture.width;
            maxY = levelTexture.height;
            grid = new Node[maxX, maxY];
            textureInstance = new Texture2D(maxX, maxY);
            textureInstance.filterMode = FilterMode.Point;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Node n = new Node();
                    n.x = x;
                    n.y = y;

                    Color c = levelTexture.GetPixel(x, y);
                    textureInstance.SetPixel(x, y, c);
                    n.isEmpty = (c.a == 0);

                    grid[x, y] = n;
                }
            }

            textureInstance.Apply();
            Rect rect = new Rect(0, 0, maxX, maxY);
            levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        }

        protected override void OnGameStart()
        {
            Macro.DisplayActionVerb("Blow Up !");
        }

        protected override void OnActionVerbDisplayEnd()
        {
            Macro.StartTimer(7);
        }

        protected override void OnBeat()
        {
            if (currentPlayerState && nbLance < 3)
            {
                Dynamite dinScript = Instantiate(dynamite, dynSpawn.position, Quaternion.identity).GetComponent<Dynamite>();
                dinScript.target = endMousePosTemp;
                dinScript.gm = this;
                dinScript.mousePos = Camera.main.WorldToScreenPoint(endMousePosTemp);
                nbLance++;
            }
            currentPlayerState = !currentPlayerState;
        }

        void Update()
        {
            GetMousePosition();
            TestTirDynamite();

            if (addFill)
            {
                DebugFill();
            }

            HandleFillNodes();
            ClearListOfPixels();

            BuildListOfNodes();

            if (applyTexture)
                textureInstance.Apply();

        }

        public void ExplodeDynamite(Vector3 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            Vector2 dynamitePosition = ray.GetPoint(5);

            List<Node> NodesToClear = new List<Node>();
            for (int i = -radius; i < radius; i++)
            {
                for (int j = -radius; j < radius; j++)
                {
                    Color c = Color.white;
                    c.a = 0;

                    float dx = (GetNodeFromWorldPos(dynamitePosition).x + i) - GetNodeFromWorldPos(dynamitePosition).x;
                    float dy = (GetNodeFromWorldPos(dynamitePosition).y + j) - GetNodeFromWorldPos(dynamitePosition).y;
                    float distanceSqured = dx * dx + dy * dy;

                    if (distanceSqured <= (radius * radius) && GetNodeFromWorldPos(dynamitePosition).x - i < grid.GetLength(0) && GetNodeFromWorldPos(dynamitePosition).x - i > 0 && GetNodeFromWorldPos(dynamitePosition).y - j < grid.GetLength(1) && GetNodeFromWorldPos(dynamitePosition).y - j > 0)
                    {
                        RaycastHit2D testRay = Physics2D.Raycast(dynamitePosition, (new Vector2(GetWorldPosFromNode(grid[(GetNodeFromWorldPos(dynamitePosition).x - i), (GetNodeFromWorldPos(dynamitePosition).y - j)]).x, GetWorldPosFromNode(grid[(GetNodeFromWorldPos(dynamitePosition).x - i), (GetNodeFromWorldPos(dynamitePosition).y - j)]).y) - dynamitePosition).normalized, Vector2.Distance(dynamitePosition, new Vector2(GetWorldPosFromNode(grid[(GetNodeFromWorldPos(dynamitePosition).x - i), (GetNodeFromWorldPos(dynamitePosition).y - j)]).x, GetWorldPosFromNode(grid[(GetNodeFromWorldPos(dynamitePosition).x - i), (GetNodeFromWorldPos(dynamitePosition).y - j)]).y)));
                        if (testRay.collider == null || testRay.collider.GetComponent<DetectionObjetIndestructible>() == null)
                        {
                            Node n = grid[GetNodeFromWorldPos(dynamitePosition).x - i, GetNodeFromWorldPos(dynamitePosition).y - j];
                            n.isEmpty = true;
                            textureInstance.SetPixel(n.x, n.y, c);
                            applyTexture = true;
                        }
                    }
                }

            }
        }

        void GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = ray.GetPoint(5);
            curNode = GetNodeFromWorldPos(mousePos);
        }

        protected override void OnTimerEnd()
        {
            foreach(bool etat in etatFilonOr)
            {
                if(etat)
                {
                    Macro.Lose();
                    Debug.Log("Lose");
                    Macro.EndGame();
                    return;
                }
            }
            Macro.Win();
            Debug.Log("Win");
            Macro.EndGame();
        }

        private Vector2 mouseStopPosition = Vector2.zero;
        private Vector2 endMousePos = Vector2.zero;

        //Test
        void TestTirDynamite()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            endMousePos = ray.GetPoint(5);
            endMousePosTemp = mouseStopPosition;
            mouseStopPosition = dynSpawn.position;
            int i = 0;
            while (GetNodeFromWorldPos(endMousePosTemp) != null && GetNodeFromWorldPos(endMousePosTemp).isEmpty && i < 100)
            {
                endMousePosTemp = endMousePosTemp + ((endMousePos - mouseStopPosition).normalized) * 0.05f;
                i++;
            }
            endMousePosTemp = endMousePosTemp - ((endMousePos - mouseStopPosition).normalized) * 0.05f;
            RaycastHit2D testRay = Physics2D.Raycast(mouseStopPosition, (endMousePosTemp - mouseStopPosition).normalized, Vector2.Distance(mouseStopPosition, endMousePosTemp));
            Debug.DrawRay(mouseStopPosition, (endMousePosTemp - mouseStopPosition), Color.blue, 1, false);

            lineRnd.SetPosition(0, new Vector3(dynSpawn.position.x, dynSpawn.position.y, -1));
            lineRnd.SetPosition(1, new Vector3(endMousePosTemp.x, endMousePosTemp.y, -1));
            //lineRnd.enabled = true;
        }

        public void ReajustRay(Dynamite dyn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector2 endMousePosTmp = dyn.transform.position;
            //endMousePosTmp += dyn.velocity.normalized;
            Vector2 endMousePosTempTmp = endMousePosTmp;
            Vector2 mouseStopPositionTmp = dyn.transform.position;
            int i = 0;
            while ((GetNodeFromWorldPos(endMousePosTempTmp) != null && !GetNodeFromWorldPos(endMousePosTempTmp).isEmpty) && i < 100)
            {
                endMousePosTempTmp = endMousePosTempTmp - (dyn.velocity.normalized.normalized) * 0.04f;
                i++;
            }
            i = 0;
            while ((GetNodeFromWorldPos(endMousePosTempTmp) == null || GetNodeFromWorldPos(endMousePosTempTmp).isEmpty) && i < 100)
            {
                endMousePosTempTmp = endMousePosTempTmp + (dyn.velocity.normalized.normalized) * 0.04f;
                i++;
            }
            endMousePosTempTmp = endMousePosTempTmp - ((endMousePosTmp - mouseStopPositionTmp).normalized) * 0.07f;
            RaycastHit2D testRay = Physics2D.Raycast(mouseStopPositionTmp, (endMousePosTempTmp - mouseStopPositionTmp).normalized, Vector2.Distance(mouseStopPositionTmp, endMousePosTempTmp));
            Debug.DrawRay(mouseStopPositionTmp, (endMousePosTempTmp - mouseStopPositionTmp), Color.blue, 1, false);

            dyn.target = endMousePosTempTmp;
            dyn.mousePos = Camera.main.WorldToScreenPoint(endMousePosTempTmp);
        }


        // Edit functions
        List<Node> clearNodes = new List<Node>();
        List<Node> buildNodes = new List<Node>();
        List<FillNode> fillNodes = new List<FillNode>();

        public void ClearListOfPixels()
        {
            if (clearNodes.Count == 0)
                return;

            Color c = Color.white;
            c.a = 0;

            for (int i = 0; i < clearNodes.Count; i++)
            {
                clearNodes[i].isEmpty = true;
                clearNodes[i].isFiller = false;
                textureInstance.SetPixel(clearNodes[i].x, clearNodes[i].y, c);
            }

            clearNodes.Clear();
            applyTexture = true;
        }

        void BuildListOfNodes()
        {
            if (buildNodes.Count == 0)
                return;

            for (int i = 0; i < buildNodes.Count; i++)
            {
                buildNodes[i].isEmpty = false;
                textureInstance.SetPixel(buildNodes[i].x, buildNodes[i].y, buildColor);
            }

            buildNodes.Clear();
            applyTexture = true;
        }

        void DebugFill()
        {
            if(pixelsOut > maxPixels)
            {
                addFill = false;
                return;
            }

            p_t += Time.deltaTime;

            if (p_t > 0.05f)
            {
                pixelsOut++;
                p_t = 0;
            }
            else
            {
                return;
            }

            Node n = GetNodeFromWorldPos(fillDebugObj.position);
            FillNode f = new FillNode();
            f.x = n.x;
            f.y = n.y;
            fillNodes.Add(f);
            applyTexture = true;
        }

        void HandleFillNodes()
        {
            f_t += Time.deltaTime;

            if(f_t > 0.05f)
            {
                f_t = 0;
            }
            else
            {
                return;
            }

            if (fillNodes.Count == 0)
                return;

            for (int i = 0; i < fillNodes.Count; i++)
            {
                FillNode f = fillNodes[i];
                Node cn = GetNode(f.x, f.y);
                cn.isFiller = true;

                int _y = f.y;
                _y -= 1;

                Node d = GetNode(f.x, _y);
                if (d == null)
                {
                    fillNodes.Remove(f);
                    continue;
                }

                if (d.isEmpty)
                {
                    d.isEmpty = false;
                   // d.isFiller = true;
                    textureInstance.SetPixel(d.x, d.y, fillColor);
                    f.y = _y;
                    clearNodes.Add(cn);
                }
                else
                {
                    Node df = GetNode(f.x - 1, _y);
                    if (df.isEmpty)
                    {
                        textureInstance.SetPixel(df.x, df.y, fillColor);
                        f.y = _y;
                        f.x -= 1;
                        df.isEmpty = false;
                    //    df.isFiller = true;
                        clearNodes.Add(cn);
                    }
                    else
                    {
                        Node bf = GetNode(f.x + 1, _y);

                        if (bf.isEmpty)
                        {
                            bf.isEmpty = false;
                          //  bf.isFiller = true;
                            textureInstance.SetPixel(bf.x, bf.y, fillColor);
                            f.y = _y;
                            f.x += 1;
                            clearNodes.Add(cn);
                        }
                        else
                        {
                            f.t++;
                            if (f.t > 15)
                            {
                                Node _cn = GetNode(f.x, f.y);
                                _cn.isFiller = false;
                                fillNodes.Remove(f);
                            }
                        }
                    }

                    /*int _x1 = (f.movingLeft) ? -1 : 1;
                    int _x2 = (f.movingLeft) ? 1 : -1;

                    Node df = GetNode(f.x + _x1, _y);
                    if (df.isEmpty)
                    {
                        df.isEmpty = false;
                        textureInstance.SetPixel(df.x, df.y, fillColor);
                        f.y = _y;
                        f.x += _x1;
                        clearNodes.Add(cn);
                    }
                    else
                    {
                        Node db = GetNode(f.x + _x2, _y);
                        if (db.isEmpty)
                        {
                            db.isEmpty = false;
                            textureInstance.SetPixel(db.x, db.y, fillColor);
                            f.y = _y;
                            f.x += _x2;
                            clearNodes.Add(db);
                        }
                        else
                        {
                            f.t++;
                            if(f.t > 5)
                            {
                                fillNodes.Remove(f);
                            }
                        }
                    }*/
                }
            }
        }

        //Node functions
        public Node GetNodeFromWorldPos(Vector3 wp)
        {
            int t_x = Mathf.RoundToInt(wp.x / posOffset);
            int t_y = Mathf.RoundToInt(wp.y / posOffset);

            return GetNode(t_x, t_y);
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
                return null;
            return grid[x, y];
        }

        public Vector3 GetWorldPosFromNode(int x, int y)
        {
            Vector3 r = Vector3.zero;
            r.x = x * posOffset;
            r.y = y * posOffset;
            return r;
        }

        public Vector3 GetWorldPosFromNode(Node n)
        {
            if (n == null)
                return -Vector3.one;

            Vector3 r = Vector3.zero;
            r.x = n.x * posOffset;
            r.y = n.y * posOffset;
            return r;
        }
    }

    public class Node
    {
        public int x;
        public int y;
        public bool isEmpty;
        public bool isStoped;
        public bool isFiller;
    }

    public class FillNode
    {
        public int x;
        public int y;
        public int t;
        public bool movingLeft;

    }
}
